import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiServiceService } from '../../services/api-service.service';
import { Tag } from '../../models/Tag';

@Component({
  selector: 'app-filters',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.scss'],
})
export class FiltersComponent {
  @Output() changed = new EventEmitter<any>();

  tags: Tag[] = [];
  sentiments = ['Positive', 'Neutral', 'Negative'];

  Search: string = '';
  selectedTag: string = '';
  selectedSentiment: string = '';

  constructor(private apiService: ApiServiceService) {}

  ngOnInit() {
    this.apiService.getTags().subscribe((res) => (this.tags = res));
  }

  onFilterChange() {
    this.Search = '';

    this.changed.emit({
      tag: this.selectedTag || null,
      sentiment: this.selectedSentiment || null,
      query: null, // ignore the semantic search
    });
  }

  submitSearch() {
    const cleaned = this.Search.trim();

    if (cleaned.length < 10) {
      alert('Please enter a full sentence or meaningful search query.');
      return;
    }

    // Reset filters when semantic search is used
    this.selectedTag = '';
    this.selectedSentiment = '';

    this.changed.emit({
      query: cleaned,
      tag: null,
      sentiment: null,
    });
  }

  clearFilters() {
    this.selectedTag = '';
    this.selectedSentiment = '';
    this.Search = '';

    this.changed.emit({
      tag: null,
      sentiment: null,
      query: null,
    });
  }
}
