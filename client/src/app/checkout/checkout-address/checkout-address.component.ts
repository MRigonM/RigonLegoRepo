import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/account/account.service';

@Component({
  selector: 'app-checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss']
})
export class CheckoutAddressComponent {
  @Input() checkOutForm? : FormGroup;

  constructor(private accountService : AccountService,
              private toastr : ToastrService) {}

  saveUserAddress() {
    this.accountService.updateUserAddress(this.checkOutForm?.get('addressForm')?.value).subscribe({
      next : () => {
        this.toastr.success('Address saved');
        this.checkOutForm?.get('addressForm')?.reset(this.checkOutForm?.get('addressForm')?.value);
      }
    })
  }

}
