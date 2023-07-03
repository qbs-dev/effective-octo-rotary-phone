import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Page, PaginationOptions } from 'src/app/models/common.model';
import { UrlVisit } from 'src/app/models/url.model';
import { UrlService } from 'src/app/services/url.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-url-visits',
  templateUrl: './url-visits.component.html',
  styleUrls: ['./url-visits.component.less'],
})
export class UrlVisitsComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator)
  urlVisitsPaginator: MatPaginator = null!;

  urlId: number = null!;

  urlVisitsPaginationOpts: PaginationOptions = {
    totalItems: 0,
    pageSize: 10,
    pageIndex: 0,
  };
  urlVisitsDataSource: MatTableDataSource<UrlVisit> = new MatTableDataSource();
  // routeParams$: Subscription = null!;
  routeQueryParams$: Subscription = null!;
  isUrlVisitsLoading: boolean = true;
  urlVisitsDisplayedColumns: string[] = [
    'visitTimestamp',
    'ipAddress',
    'countryCode',
  ];

  constructor(
    private urlService: UrlService,
    private snack: SnackNotifyComponent,
    private errorService: HttpErrorResponseService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngAfterViewInit() {
    this.urlVisitsDataSource.paginator = this.urlVisitsPaginator;
  }

  ngOnInit(): void {
    this.urlId = this.route.snapshot.params['url-id'];
    // this.routeParams$ = this.route.params.subscribe((params) => {
    //   const routeParams = {
    //     urlId: params['url-id'],
    //   };
    //   this.urlId = routeParams.urlId;
    // });

    this.routeQueryParams$ = this.route.queryParams.subscribe((params) => {
      const queryParams = {
        page: params['page'],
      };

      if (queryParams.page) {
        this.urlVisitsPaginationOpts.pageIndex = queryParams.page - 1;
      }
      this.getUrlVisitsPage(this.urlId);
    });
  }

  ngOnDestroy(): void {
    // this.routeParams$.unsubscribe();
    this.routeQueryParams$.unsubscribe();
  }

  getUrlVisitsPage(urlId: number) {
    this.isUrlVisitsLoading = true;
    this.urlService
      .getUrlVisitsPage(
        urlId,
        '-VisitTimestamp',
        '',
        this.urlVisitsPaginationOpts.pageIndex + 1,
        this.urlVisitsPaginationOpts.pageSize
      )
      .subscribe({
        next: (res: Page<UrlVisit>) => {
          this.urlVisitsDataSource.data = res.pageItems;
          this.urlVisitsPaginationOpts.totalItems = res.totalCount;
          this.isUrlVisitsLoading = false;
        },
        error: (error) => {
          this.snack.openSnackBar(
            'Error: ' + this.errorService.getStringFromError(error),
            'OK'
          );
          this.isUrlVisitsLoading = false;
        },
      });
  }

  urlVisitsPageChanged(event: PageEvent) {
    this.urlVisitsPaginationOpts.pageIndex = event.pageIndex;
    this.urlVisitsPaginationOpts.pageSize = event.pageSize;
    this.router.navigate([`/urls/${this.urlId}/visits`], {
      queryParams: {
        page: this.urlVisitsPaginationOpts.pageIndex + 1,
      },
    });
  }
}
