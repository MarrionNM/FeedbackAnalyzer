import { Component, inject, OnInit } from '@angular/core';
import { ApiServiceService } from '../../services/api-service.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FiltersComponent } from '../../components/filters/filters.component';
import { LoaderComponent } from '../../components/loader/loader.component';
import { PaginationComponent } from '../../components/pagination/pagination.component';
import { TagComponent } from '../../components/tag/tag.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-feedback-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TagComponent,
    LoaderComponent,
    FiltersComponent,
    PaginationComponent,
  ],
  templateUrl: './feedback-list.component.html',
  styleUrls: ['./feedback-list.component.scss'],
})
export class FeedbackListComponent implements OnInit {
  feedbacks: any[] = [];
  loading = false;

  // Filters
  tag: string = '';
  sentiment: string = '';
  Search: string = '';

  // Pagination
  pageNumber = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 1;

  private router = inject(Router);

  constructor(private api: ApiServiceService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;

    this.api
      .getFeedbackList({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sentiment: this.sentiment,
        tag: this.tag,
        search: this.Search,
      })
      .subscribe((res) => {
        this.loading = false;
        this.feedbacks = res.data.data;
        console.log('data:', typeof res.data.currentPage, res.data.currentPage);
        console.log(
          'CurrentPage:',
          typeof res.data.currentPage,
          res.data.currentPage
        );
        console.log(
          'TotalPages:',
          typeof res.data.totalPages,
          res.data.totalPages
        );

        this.pageNumber = res.data.currentPage!;
        this.pageSize = res.data.pageSize!;
        this.totalRecords = res.data.totalRecords!;
        this.totalPages = res.data.totalPages!;
      });
  }

  onFiltersChanged(filters: any) {
    this.tag = filters.tag;
    this.sentiment = filters.sentiment;
    this.Search = filters.query;
    this.pageNumber = 1;
    this.load();
  }

  onPageChanged(page: number) {
    this.pageNumber = page;
    this.load();
  }

  openFeedbackDetails(id: string) {
    this.router.navigate([`/feedback/${id}`]);
  }
}
