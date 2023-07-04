export interface ArdalisResultError {
  title: string;
  status: number;
  detail: string;
}

export interface MessageResponse {
  message: string;
}

export interface Page<Type> {
  pageItems: Type[];
  totalCount: number;
}

export interface PaginationOptions {
  totalItems: number;
  pageSize: number;
  pageIndex: number;
}

export interface PieChartOptions {
  labelData: any;
  datasetData: any;
  datasetLabel?: string;
}
