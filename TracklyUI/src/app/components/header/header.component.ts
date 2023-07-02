import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent
{
  isMoblieScreen: boolean = false;

  constructor(breakpointObserver: BreakpointObserver, router: Router)
  {
    breakpointObserver.observe([
      Breakpoints.XSmall,
      Breakpoints.Small,
      Breakpoints.Medium,
    ]).subscribe(result =>
    {
      if (result.matches)
      {
        this.isMoblieScreen = true;
      } else
      {
        this.isMoblieScreen = false;
      }
    });
  }
}
