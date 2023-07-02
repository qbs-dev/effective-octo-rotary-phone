import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { UrlService } from 'src/app/services/url.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { AuthService } from 'src/app/services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { EditUrlDialogComponent } from '../edit-url-dialog/edit-url-dialog.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Page, PaginationOptions } from 'src/app/models/common.model';
import { Url, UrlShort } from 'src/app/models/url.model';

@Component({
  selector: 'app-manage-urls',
  templateUrl: './manage-urls.component.html',
  styleUrls: ['./manage-urls.component.less'],
})
export class ManageUrlsComponent implements OnInit, OnDestroy {
  urls: any;
  isLoading: boolean = true;
  isError: boolean = true;
  sortingControl = new FormControl<string>('');

  paginationOpts: PaginationOptions = {
    pageIndex: 1,
    pageSize: 10,
    totalItems: 0,
  };

  routeParams$: Subscription = null!;
  onSortChange$: Subscription = null!;
  isLoggedIn = this.authService.isLoggedIn;

  constructor(
    private urlService: UrlService,
    private authService: AuthService,
    private errorService: HttpErrorResponseService,
    private snack: SnackNotifyComponent,
    private route: ActivatedRoute,
    private router: Router,
    private addUrlDialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.routeParams$ = this.route.queryParams.subscribe((params) => {
      const routeParams = {
        order: params['order'],
        page: params['page'],
      };

      if (routeParams.order) {
        this.sortingControl.setValue(routeParams.order, { emitEvent: false });
      } else {
        this.sortingControl.setValue('-CreatedAt', { emitEvent: false });
      }

      if (routeParams.page) {
        this.paginationOpts.pageIndex = routeParams.page;
      }

      this.getUrlsPage();
    });

    this.onSortChange$ = this.sortingControl.valueChanges.subscribe((value) => {
      this.router.navigate(['/urls'], {
        queryParams: {
          order: value,
          page: 1,
        },
      });
    });
  }

  ngOnDestroy(): void {
    if (this.routeParams$) this.routeParams$.unsubscribe();
    if (this.onSortChange$) this.onSortChange$.unsubscribe();
  }

  getUrlsPage(): void {
    this.isLoading = true;
    const order = this.sortingControl.value!;
    this.urlService
      .getUrlsPage(
        order,
        this.paginationOpts.pageIndex,
        this.paginationOpts.pageSize
      )
      .subscribe({
        next: (res: Page<UrlShort>) => {
          this.urls = res.pageItems;
          this.paginationOpts.totalItems = res.totalCount;
          this.isLoading = false;
          this.isError = false;
        },
        error: (error) => {
          this.urls = [];
          this.snack.openSnackBar(
            'Error: ' + this.errorService.getStringFromError(error),
            'OK'
          );
          this.isLoading = false;
          this.isError = true;
        },
      });
  }

  changePage(newPage: number) {
    this.router.navigate(['/urls'], {
      queryParams: {
        order: this.sortingControl.value,
        page: newPage,
      },
    });
  }

  getUrlStatusTooltip(url: Url): string {
    return 'Link status: ' + (url.isActive ? 'active' : 'inactive');
  }

  getUrlStatusIcon(url: Url): string {
    return url.isActive ? 'link' : 'link_off';
  }

  openAddUrlDialog() {
    this.addUrlDialog.open(EditUrlDialogComponent, {
      data: {
        isAdding: true,
      },
      maxWidth: '90vw',
      maxHeight: '90vh',
    });
  }
}
