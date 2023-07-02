import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sentencecase'
})
export class SentencecasePipe implements PipeTransform
{

  transform(value: string): string
  {
    return value[0].toUpperCase() + value.slice(1);
  }
}
