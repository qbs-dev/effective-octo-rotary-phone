import { Component, Input } from '@angular/core';
import { MessageResponse } from '../../models/common.model';
import { AuthService } from '../../services/auth.service';
import { HttpErrorResponseService } from '../../services/http-error-response.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';

@Component({
  selector: 'app-header-nav-menu',
  templateUrl: './header-nav-menu.component.html',
  styleUrls: ['./header-nav-menu.component.less'],
})
export class HeaderNavMenuComponent {
  isLoggedIn = this.authService.isLoggedIn;

  constructor(
    private authService: AuthService,
    private errorService: HttpErrorResponseService,
    private snack: SnackNotifyComponent
  ) {}
  @Input() showMobileMenu: boolean = null!;

  logout() {
    this.authService.logout().subscribe(
      (response: MessageResponse) => {
        this.authService.forceLogoutCleanup();
        this.snack.openSnackBar('Logged out successfully', 'OK');
      },
      (error) => {
        this.authService.forceLogoutCleanup();
        this.snack.openSnackBar(
          `Error: ` + this.errorService.getStringFromError(error),
          'OK'
        );
      }
    );
  }
}
