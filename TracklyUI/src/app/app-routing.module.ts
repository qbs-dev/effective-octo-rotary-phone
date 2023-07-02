import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AboutUsComponent } from './components/about-us/about-us.component';
import { PageForbiddenComponent } from './components/page-forbidden/page-forbidden.component';
import { hasLoggedInAuthGuard } from './helpers/auth.guard';
import { ManageUrlsComponent } from './components/manage-urls/manage-urls.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'urls', component: ManageUrlsComponent },
  // { path: 'urls/:url-id', component: UrlDetailsComponent, canActivate: [hasLoggedInAuthGuard()] },
  // { path: 'urls/:url-id:/visits', component: UrlVisitsComponent, canActivate: [hasLoggedInAuthGuard()] },
  // { path: 'urls/:url-id:/stats', component: UrlStatsComponent, canActivate: [hasLoggedInAuthGuard()] },
  { path: 'about-us', component: AboutUsComponent },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [hasLoggedInAuthGuard()],
  },
  { path: '', redirectTo: '/about-us', pathMatch: 'full' },
  { path: 'forbidden', component: PageForbiddenComponent },
  { path: 'not-found', component: PageNotFoundComponent },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
