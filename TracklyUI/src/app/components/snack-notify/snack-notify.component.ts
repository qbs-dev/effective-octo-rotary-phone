import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SentencecasePipe } from '../../pipes/sentencecase.pipe';

@Component({
  selector: 'app-snack-notify',
  templateUrl: './snack-notify.component.html',
  styleUrls: ['./snack-notify.component.less']
})
export class SnackNotifyComponent
{
  durationInSeconds = 3;
  constructor(private snackBar: MatSnackBar, private sentencecase: SentencecasePipe) { }

  openSnackBar(message: string, action: string)
  {
    this.snackBar.open(this.sentencecase.transform(message), action, { duration: this.durationInSeconds * 1000 });
  }
}
