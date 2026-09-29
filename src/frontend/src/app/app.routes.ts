import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Listings } from './features/listings/listings';
import { NotFound } from './features/not-found/not-found';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'listings', component: Listings },
  { path: '**', component: NotFound },
];
