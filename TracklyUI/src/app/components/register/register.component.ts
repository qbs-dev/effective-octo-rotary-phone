import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { RegisterRequest } from 'src/app/models/profile.model';
import { ProfileService } from 'src/app/services/profile.service';
import { Router } from '@angular/router';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import {
  createPasswordStrengthValidator,
  createConfirmPasswordValidator,
  getErrorMessage,
} from 'src/app/helpers/custom-validators';
import { MessageResponse } from '../../models/common.model';
import { HttpErrorResponseService } from '../../services/http-error-response.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.less'],
})
export class RegisterComponent {
  isSending = false;
  email = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(128),
    Validators.email,
  ]);
  password = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(128),
    createPasswordStrengthValidator(),
  ]);
  confirmPassword = new FormControl('', [
    Validators.required,
    createConfirmPasswordValidator(),
  ]);
  firstName = new FormControl('', [
    Validators.required,
    Validators.minLength(2),
    Validators.maxLength(64),
  ]);
  middleName = new FormControl('', [
    Validators.minLength(2),
    Validators.maxLength(64),
  ]);
  lastName = new FormControl('', [
    Validators.minLength(2),
    Validators.maxLength(64),
  ]);
  registerForm: FormGroup;
  getErrorMessage = getErrorMessage;

  constructor(
    private formBuilder: FormBuilder,
    private profileService: ProfileService,
    private errorService: HttpErrorResponseService,
    private router: Router,
    private snack: SnackNotifyComponent
  ) {
    this.registerForm = this.formBuilder.group({
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
      firstName: this.firstName,
      middleName: this.middleName,
      lastName: this.lastName,
    });
  }

  sendForm() {
    if (!this.isSending) {
      this.isSending = true;
      if (this.registerForm.valid) {
        const formValue = this.registerForm.value;
        const registerRequest: RegisterRequest = {
          email: formValue.email,
          password: formValue.password,
          firstName: formValue.firstName,
          middleName: formValue.middleName,
          lastName: formValue.lastName,
        };
        this.profileService.register(registerRequest).subscribe(
          (response: MessageResponse) => {
            this.isSending = false;
            this.snack.openSnackBar(response.message, 'OK');
            this.router.navigate(['/login']);
          },
          (error) => {
            this.isSending = false;
            this.snack.openSnackBar(
              `Error: ` + this.errorService.getStringFromError(error),
              'OK'
            );
            console.log(error.message);
          }
        );
      }
    }
  }
}
