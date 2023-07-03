import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { MaterialModule } from './material/material.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgxPaginationModule } from 'ngx-pagination';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { SnackNotifyComponent } from './components/snack-notify/snack-notify.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FooterComponent } from './components/footer/footer.component';
import { HeaderComponent } from './components/header/header.component';
import { HeaderNavMenuComponent } from './components/header-nav-menu/header-nav-menu.component';
import { ProfileComponent } from './components/profile/profile.component';
import { EditProfileDialogComponent } from './components/edit-profile-dialog/edit-profile-dialog.component';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';
import { AuthInterceptor } from './helpers/auth.interceptor';
import { AboutUsComponent } from './components/about-us/about-us.component';
import { SentencecasePipe } from './pipes/sentencecase.pipe';
import { PageForbiddenComponent } from './components/page-forbidden/page-forbidden.component';
import { ManageUrlsComponent } from './components/manage-urls/manage-urls.component';
import { EditUrlDialogComponent } from './components/edit-url-dialog/edit-url-dialog.component';
import { UrlPathInputComponent } from './components/url-path-input/url-path-input.component';
import { UrlDetailsComponent } from './components/url-details/url-details.component';
import { UrlVisitsComponent } from './components/url-visits/url-visits.component';
import { UrlStatsComponent } from './components/url-stats/url-stats.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    SnackNotifyComponent,
    FooterComponent,
    HeaderComponent,
    HeaderNavMenuComponent,
    ProfileComponent,
    EditProfileDialogComponent,
    AboutUsComponent,
    SentencecasePipe,
    PageForbiddenComponent,
    PageNotFoundComponent,
    ManageUrlsComponent,
    UrlPathInputComponent,
    EditUrlDialogComponent,
    UrlDetailsComponent,
    UrlVisitsComponent,
    UrlStatsComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    NgxPaginationModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    MatNativeDateModule,
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'ru-RU' },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    SnackNotifyComponent,
    SentencecasePipe,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
