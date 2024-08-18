import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Product } from '../shared/models/product';
import { ShopService } from './shop.service';
import { Brands } from '../shared/models/brands';
import { Types } from '../shared/models/types';
import { ShopParams } from '../shared/models/shopParams';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit{
  @ViewChild('search') searchTerms?: ElementRef;
  products : Product [] = [];
  brands : Brands [] = [];
  types : Types [] = [];

  shopParams = new ShopParams();
  sortOptions = [
    {name: 'Alphabetical' ,value:'name'},
    {name: 'Price: Low to high',value:'priceAsc'},
    {name: 'Price: High to low',value:'priceDesc'},
  ];
  totalcount = 0;
  constructor(private shopService : ShopService){}

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();

  }

  getProducts(){

    this.shopService.getProducts(this.shopParams).subscribe({
      next: response => {
        this.products = response.data;
        this.shopParams.pageNumber = response.pageIndex;
        this.shopParams.pageSize = response.pageSize;
        this.totalcount = response.count;
      },
      error : error => console.log(error)
    })
  }
  getTypes(){
  
    this.shopService.getTypes().subscribe({
      next: response => this.types = [{id : 0,name : 'All'}, ...response],
      error : error => console.log(error)
    })
  }
  getBrands(){
  
    this.shopService.getBrands().subscribe({
      next: response => this.brands = [{id : 0,name : 'All'}, ...response],
      error : error => console.log(error)
    })
  }
  onBrandSelected(brandId: number){
    this.shopParams.brandId = brandId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }
  onTypeSelected(typeId: number){
    this.shopParams.typeId = typeId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }
  onSortSelected(event: any){
    this.shopParams.sort = event.target.value;
    this.getProducts();
  }

  onPageChanged(event : any){
    if(this.shopParams.pageNumber != event.pageNumber){
      this.shopParams.pageNumber =event;
      this.getProducts();
    }
  }
  onSearch(){
      this.shopParams.search = this.searchTerms?.nativeElement.value;
      this.shopParams.pageNumber = 1;
      this.getProducts();
  }
  onReset(){
    if(this.searchTerms) this.searchTerms.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.getProducts();
  }

}
