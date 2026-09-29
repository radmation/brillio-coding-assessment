import { CurrencyPipe, DatePipe, PercentPipe, TitleCasePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { Listing, ListingsService } from '../../core/services/listings';

@Component({
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, PercentPipe, TitleCasePipe],
  selector: 'app-listings',
  styleUrl: './listings.scss',
  templateUrl: './listings.html',
  standalone: true,
})
export class Listings implements OnInit {
  private readonly listingsService = inject(ListingsService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly pageSize = 5;
  protected readonly page = signal(1);
  protected readonly listings = signal<Listing[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);
  protected readonly errors = signal<string[]>([]);

  protected readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize))
  );

  protected readonly pageNumbers = computed(() =>
    Array.from({ length: this.totalPages() }, (_, i) => i + 1)
  );

  protected readonly filters = new FormGroup({
    targetBudget: new FormControl<number | null>(null),
    minPrice: new FormControl<number | null>(null),
    maxPrice: new FormControl<number | null>(null),
    minBedrooms: new FormControl<number | null>(null),
    city: new FormControl('', { nonNullable: true }),
    search: new FormControl('', { nonNullable: true }),
  });

  ngOnInit(): void {
    this.loadListings();

    console.log(this.listings());

    this.filters.valueChanges
      .pipe(debounceTime(300), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.page.set(1);
        this.loadListings();
      });
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }

    this.page.set(page);
    this.loadListings();
  }

  protected statusBadgeClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'text-bg-warning';
      case 'sold':
        return 'text-bg-danger';
      case 'active':
        return 'text-bg-success';
      default:
        return 'text-bg-secondary';
    }
  }

  private loadListings(): void {
    const value = this.filters.getRawValue();
    this.loading.set(true);
    this.errors.set([]);

    this.listingsService
      .getListings({
        targetBudget: this.toOptionalNumber(value.targetBudget),
        minPrice: this.toOptionalNumber(value.minPrice),
        maxPrice: this.toOptionalNumber(value.maxPrice),
        minBedrooms: this.toOptionalNumber(value.minBedrooms),
        city: value.city.trim() || undefined,
        search: value.search.trim() || undefined,
        page: this.page(),
        pageSize: this.pageSize,
      })
      .subscribe({
        next: (response) => {
          console.log(response);
          this.listings.set(response.listings);
          this.totalCount.set(response.totalCount);
          this.loading.set(false);
        },
        error: (err: HttpErrorResponse) => {
          this.errors.set(this.readApiErrors(err));
          this.loading.set(false);
        },
      });
  }

  // TODO: Consider moving to utility function/class.
  private toOptionalNumber(value: number | null): number | undefined {
    return value == null || Number.isNaN(value) ? undefined : value;
  }

  private readApiErrors(err: HttpErrorResponse): string[] {
    const fieldErrors = err.error?.errors as Record<string, string[]> | undefined;
    if (fieldErrors) {
      return Object.values(fieldErrors).flat();
    }

    return [err.error?.title ?? err.message ?? 'Request failed.'];
  }
}
