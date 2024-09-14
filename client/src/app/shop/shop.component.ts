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

  shopParams : ShopParams;
  sortOptions = [
    {name: 'Alphabetical' ,value:'name'},
    {name: 'Price: Low to high',value:'priceAsc'},
    {name: 'Price: High to low',value:'priceDesc'},
  ];
  totalcount = 0;
  constructor(private shopService: ShopService) {
    this.shopParams = shopService.getShopParams();
  }

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();

  }

  getProducts(){

    this.shopService.getProducts().subscribe({
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
    const params = this.shopService.getShopParams();
    params.brandId = brandId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }
  onTypeSelected(typeId: number){
    const params = this.shopService.getShopParams();
    params.typeId = typeId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }
  onSortSelected(event: any){
    const params = this.shopService.getShopParams();
    params.sort = event.target.value;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }

  onPageChanged(event : any){
    const params = this.shopService.getShopParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.shopService.setShopParams(params);
      this.shopParams = params;
      this.getProducts();
    }
  }
  onSearch(){
    const params = this.shopService.getShopParams();
    params.search = this.searchTerms?.nativeElement.value;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.shopParams = params;
      this.getProducts();
  }
  onReset(){
    if(this.searchTerms) this.searchTerms.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }

}
