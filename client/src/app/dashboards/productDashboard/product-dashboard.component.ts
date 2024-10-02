import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/shared/models/product';
import { ProductService } from './product-dashboard.service';
import {FormBuilder, FormsModule} from '@angular/forms';
import { CommonModule } from '@angular/common';
import {ProductToCreate} from "../../shared/models/productToCreate";
import {Brands} from "../../shared/models/brands";
import {Types} from "../../shared/models/types";
import {ShopService} from "../../shop/shop.service";
import {ShopParams} from "../../shared/models/shopParams";
import {SharedModule} from "../../shared/shared.module";

@Component({
  selector: 'app-product-dashboard',
  templateUrl: './product-dashboard.component.html',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    SharedModule
  ],
  styleUrls: ['./product-dashboard.component.scss']
})
export class ProductDashboardComponent implements OnInit {
    products : Product [] = [];
    brands : Brands [] = [];
    types : Types [] = [];

    shopParams : ShopParams;

  ngOnInit(): void {
      this.getProducts();
      this.getTypes();
      this.getBrands();
    }

  totalcount = 0;
  constructor(private fb : FormBuilder,private shopService: ShopService,
              private productService: ProductService) {
    this.shopParams = shopService.getShopParams();
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

  onPageChanged(event : any){
    const params = this.shopService.getShopParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.shopService.setShopParams(params);
      this.shopParams = params;
      this.getProducts();
    }
  }

  newProduct = this.fb.group({
    name: [''],
    price: [null],
    description: [''],
    productType: [null],
    productBrand: [null],
    pictureUrl: ['']
  });
  // Create a new product using the service
  onCreateProduct() {
    this.productService.createProduct(this.newProduct.value).subscribe({
      next: () => this.getProducts(),
      error: (err) => console.error('Failed to create product', err)
    });
  }

  // Update the existing product using the service
  onUpdateProduct(product: Product) {
    this.productService.updateProduct(product.id, product).subscribe({
      next: () => this.getProducts(),  // Reload the products after updating
      error: (err) => console.error('Failed to update product', err)
    });
  }

  // Delete a product using the service
  onDeleteProduct(id: number) {
    this.productService.deleteProduct(id).subscribe({
      next: () => this.getProducts(),  // Reload the products after deleting
      error: (err) => console.error('Failed to delete product', err)
    });
  }

  getTypes(){
    this.shopService.getTypes().subscribe({
      next: response => this.types = [{id : 0,name : 'Select brand'}, ...response],
      error : error => console.log(error)
    })
  }
  getBrands(){
    this.shopService.getBrands().subscribe({
      next: response => this.brands = [{id : 0,name : 'Select type'}, ...response],
      error : error => console.log(error)
    })
  }
}
