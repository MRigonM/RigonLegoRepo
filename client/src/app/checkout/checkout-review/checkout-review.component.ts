import {Component, Input} from '@angular/core';
import {BasketService} from "../../basket/basket.service";
import {ToastrService} from "ngx-toastr";
import {CdkStepper} from "@angular/cdk/stepper";

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent {
  @Input() appStepper?: CdkStepper;

  constructor(private basketService: BasketService, private toastr: ToastrService) {}

  createPaymentIntent() {
    this.basketService.createPaymentIntent().subscribe({
      next: () => {
        this.appStepper?.next();
      },
      error: error => this.toastr.error(error.message)
    })
  }

}
