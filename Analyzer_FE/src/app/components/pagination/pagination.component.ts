import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss'],
})
export class PaginationComponent {
  @Input() pageNumber = 1; // current page
  @Input() totalPages = 1; // total pages from backend

  @Output() pageChanged = new EventEmitter<number>();

  // Build page array for UI
  get pages(): (number | string)[] {
    const total = this.totalPages;
    const current = this.pageNumber;

    // CASE 1: total pages ≤ 5 → show all pages
    if (total <= 5) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    let result: (number | string)[] = [];

    // Always show first page
    result.push(1);

    // Left ellipsis
    if (current > 3) {
      result.push('…');
    }

    // Middle range: current-1, current, current+1
    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);

    for (let p = start; p <= end; p++) {
      result.push(p);
    }

    // Right ellipsis
    if (current < total - 2) {
      result.push('…');
    }

    // Always show last page
    result.push(total);

    return result;
  }

  changePage(page: number | string) {
    if (typeof page === 'number') {
      this.pageChanged.emit(page);
    }
  }

  next() {
    if (this.pageNumber < this.totalPages) {
      this.pageChanged.emit(this.pageNumber + 1);
    }
  }

  prev() {
    if (this.pageNumber > 1) {
      this.pageChanged.emit(this.pageNumber - 1);
    }
  }
}
