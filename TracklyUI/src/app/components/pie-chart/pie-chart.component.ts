import { AfterViewInit, Component, Input, Output } from '@angular/core';
import Chart from 'chart.js/auto';
import { PieChartOptions } from 'src/app/models/common.model';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.less'],
})
export class PieChartComponent implements AfterViewInit {
  @Input()
  chartId!: string;
  @Input()
  chartOptions: any;

  chart: any;

  constructor() {}

  ngAfterViewInit(): void {
    this.chart = this.buildPieChart(this.chartId, this.chartOptions);
  }

  buildPieChart(chartId: string, chartOptions: PieChartOptions): any {
    const ctx = (<HTMLCanvasElement>(
      document.getElementById(chartId)!
    )).getContext('2d')!;

    return new Chart(ctx, {
      type: 'pie',
      data: {
        labels: chartOptions.labelData,
        datasets: [
          {
            label: chartOptions.datasetLabel,
            data: chartOptions.datasetData,
          },
        ],
      },
      options: {
        resizeDelay: 100,
      },
    });
  }
}
