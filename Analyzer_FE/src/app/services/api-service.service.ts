import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { Feedback } from '../models/Feedback';
import { PagedResult } from '../models/PagedResult';
import { Tag } from '../models/Tag';
import { DataResponse } from '../models/DataResponse';

@Injectable({
  providedIn: 'root',
})
export class ApiServiceService {
  private base = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  submitFeedback(payload: any) {
    return this.http.post(`${this.base}/feedback`, payload).pipe(
      catchError((err: HttpErrorResponse) => {
        let message = 'Something went wrong. Please try again.';

        // Backend returned a JSON error?
        if (err.error?.message) {
          message = err.error.message;
        }

        // Custom BE Error Response
        if (err.error?.errors) {
          message = err.error.errors.join(', ');
        }

        return throwError(() => new Error(message));
      })
    );
  }

  getFeedbackList(params: {
    pageNumber?: number;
    pageSize?: number;
    sentiment?: string;
    tag?: string;
    search?: string;
  }): Observable<DataResponse<PagedResult<Feedback>>> {
    let httpParams = new HttpParams();
    if (params.pageNumber != null)
      httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize != null)
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params.sentiment)
      httpParams = httpParams.set('sentiment', params.sentiment);
    if (params.tag) httpParams = httpParams.set('tag', params.tag);
    if (params.search) httpParams = httpParams.set('search', params.search);

    return this.http.get<DataResponse<PagedResult<Feedback>>>(
      `${this.base}/feedback`,
      {
        params: httpParams,
      }
    );
  }

  getFeedback(id: string): Observable<DataResponse<Feedback>> {
    return this.http.get<DataResponse<Feedback>>(`${this.base}/feedback/${id}`);
  }

  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.base}/tag`);
  }
}
