import { Component, ViewChild } from '@angular/core';
import {
  FormControl,
  FormGroup,
  UntypedFormBuilder,
  Validators,
} from '@angular/forms';
import { AuthRequest, AuthResponse } from 'src/app/models/auth.model';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import {
  createPasswordStrengthValidator,
  getErrorMessage,
} from 'src/app/helpers/custom-validators';
import { HttpErrorResponseService } from '../../services/http-error-response.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
})
export class LoginComponent {
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
  loginForm: FormGroup;
  getErrorMessage = getErrorMessage;

  constructor(
    private formBuilder: UntypedFormBuilder,
    private authService: AuthService,
    private errorService: HttpErrorResponseService,
    private router: Router,
    private snack: SnackNotifyComponent
  ) {
    this.loginForm = this.formBuilder.group({
      email: this.email,
      password: this.password,
    });
  }

  sendForm() {
    if (!this.isSending) {
      this.isSending = true;
      if (this.loginForm.valid) {
        const formValue = this.loginForm.value;
        const authRequest: AuthRequest = {
          email: formValue.email,
          password: formValue.password,
          fingerprint: localStorage.getItem('fingerprint')!,
        };
        this.authService.login(authRequest).subscribe(
          (response: AuthResponse) => {
            this.isSending = false;
            localStorage.setItem('access', response.accessToken);
            const jwtPayload = this.authService.parseJwt(response.accessToken);
            localStorage.setItem('userId', jwtPayload.userId);
            this.snack.openSnackBar(response.message, 'OK');
            this.router.navigate(['/urls']);
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
