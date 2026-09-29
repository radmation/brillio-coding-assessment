import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface GetListingsFilters {
  minPrice?: number;
  maxPrice?: number;
  minBedrooms?: number;
  city?: string;
  search?: string;
  targetBudget?: number;
  page?: number;
  pageSize?: number;
}

export interface Listing {
  id: string;
  source: string;
  address: string;
  city: string;
  state: string;
  zip: string;
  price: number;
  bedrooms: number;
  bathrooms: number;
  sqft: number;
  latitude: number;
  longitude: number;
  listedDate: string;
  status: string;
  description: string;
  score?: number | null;
}

export interface GetListingsResponse {
  listings: Listing[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root',
})
export class ListingsService {
  private http = inject(HttpClient);
  // TODO: Move this to an environment file / build-time env var (e.g. Angular
  // fileReplacements, or NG_APP_* via @ngx-env/builder / custom webpack define).
  // Browser requests must use the host-mapped port, not the Docker service name.
  private baseUrl = 'http://localhost:5001';

  getListings(filters: GetListingsFilters = {}): Observable<GetListingsResponse> {
    let params = new HttpParams();

    if (filters.minPrice != null) {
      params = params.set('minPrice', filters.minPrice);
    }

    if (filters.maxPrice != null) {
      params = params.set('maxPrice', filters.maxPrice);
    }

    if (filters.minBedrooms != null) {
      params = params.set('minBeds', filters.minBedrooms);
    }

    if (filters.city) {
      params = params.set('city', filters.city);
    }

    if (filters.search) {
      params = params.set('search', filters.search);
    }

    if (filters.targetBudget != null) {
      params = params.set('targetBudget', filters.targetBudget);
    }

    if (filters.page != null) {
      params = params.set('page', filters.page);
    }

    if (filters.pageSize != null) {
      params = params.set('pageSize', filters.pageSize);
    }

    return this.http.get<GetListingsResponse>(`${this.baseUrl}/api/listings`, { params });
  }
}
