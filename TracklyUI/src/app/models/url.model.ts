export interface UrlBase {
  isActive: boolean,
  newPath: string,
  targetUrl: string,
}

export interface UrlEditRequest extends UrlBase {
  id: number,
  description: string
}

export interface UrlShort extends UrlBase {
  id: number,
  totalClicks: number
}

export interface Url extends UrlShort {
  description: string
  createdAt: Date
}

export interface UrlVisit {
  visitTimestamp: Date,
  ipAddress: string,
  countryCode: string
}

export interface UrlStatsResponse<Type extends VisitsByCountry | VisitsByIpAddress> {
  url: UrlShort,
  totalVisitsCount: number,
  stats: Type[]
}

export interface VisitsByCountry {
  countryCode: string,
  visitsCount: number
}

export interface VisitsByIpAddress {
  ipAddress: string,
  visitsCount: number
}
