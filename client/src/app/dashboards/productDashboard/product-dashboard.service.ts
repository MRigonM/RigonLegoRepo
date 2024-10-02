import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import { Product } from 'src/app/shared/models/product';
import {environment} from "../../../environments/environment";
import { catchError, tap } from 'rxjs/operators';
import {ProductToCreate} from "../../shared/models/productToCreate";
@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = environment.apiUrl;

  private productsSource = new BehaviorSubject<Product[]>([]);
  productsSource$ = this.productsSource.asObservable();

  private productSource = new BehaviorSubject<Product | null>(null);
  productSource$ = this.productSource.asObservable();

  private productsSource2 = new BehaviorSubject<ProductToCreate[]>([]);
  productsSource2$ = this.productsSource.asObservable();

  private productSource2 = new BehaviorSubject<ProductToCreate | null>(null);
  productSource2$ = this.productSource.asObservable();

  constructor(private http: HttpClient) { }

  // Get a single product by id
  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(this.baseUrl + `products/${id}`).pipe(
      tap(product => this.productSource.next(product)),
      catchError(error => {
        console.error(`Error fetching product with id ${id}:`, error);
        throw error;
      })
    );
  }

  // Create a new product
  createProduct(values : any){
    return this.http.post<ProductToCreate>(this.baseUrl + 'products', values).pipe(
      tap(newProduct => {
        const currentProducts = this.productsSource2.value;
        this.productsSource2.next([...currentProducts, newProduct]); // Add new product to state
      }),
      catchError(error => {
        console.error('Error creating product:', error);
        throw error;
      })
    );
  }

  // Update an existing product
  updateProduct(id: number, updatedProduct: ProductToCreate): Observable<ProductToCreate> {
    return this.http.put<ProductToCreate>(this.baseUrl + `products/${id}`, updatedProduct).pipe(
      tap((updatedProductFromServer) => {
        const updatedProducts = this.productsSource2.value.map(product =>
          product.id === id ? updatedProductFromServer : product
        );
        this.productsSource2.next(updatedProducts); // Update product in state
      }),
      catchError(error => {
        console.error(`Error updating product with id ${id}:`, error);
        throw error;
      })
    );
  }


  // Delete a product
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + `products/${id}`).pipe(
      tap(() => {
        const filteredProducts = this.productsSource.value.filter(p => p.id !== id);
        this.productsSource.next(filteredProducts); // Remove product from state
      }),
      catchError(error => {
        console.error(`Error deleting product with id ${id}:`, error);
        throw error;
      })
    );
  }
}
