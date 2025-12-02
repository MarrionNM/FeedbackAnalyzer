import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-tag',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tag.component.html',
  styleUrls: ['./tag.component.scss'],
})
export class TagComponent {
  @Input() text: string = '';
  @Input() type: 'sentiment' | 'tag' | 'priority' | string = 'tag';

  badgeClass = '';
  friendlyText = '';

  ngOnInit() {
    this.setBadgeClass();
    this.setFriendlyText();
  }

  private setBadgeClass() {
    if (!this.text) return;

    const value = this.text.toLowerCase();

    switch (this.type) {
      case 'sentiment':
        this.badgeClass =
          value === 'negative'
            ? 'badge-red'
            : value === 'neutral'
            ? 'badge-gray'
            : 'badge-primary';
        break;

      case 'priority':
        // mapping P0–P3
        this.badgeClass =
          value === 'p3'
            ? 'badge-red' // critical
            : value === 'p2'
            ? 'badge-primary' // medium
            : value === 'p1'
            ? 'badge-gray' // low
            : 'badge-light'; // P0 – minor
        break;

      case 'tag':
      default:
        this.badgeClass = 'badge-default';
        break;
    }
  }

  private setFriendlyText() {
    if (this.type !== 'priority') {
      this.friendlyText = this.text;
      return;
    }

    switch (this.text.toLowerCase()) {
      case 'p3':
        this.friendlyText = 'Critical';
        break;
      case 'p2':
        this.friendlyText = 'High Impact';
        break;
      case 'p1':
        this.friendlyText = 'Low Impact';
        break;
      case 'p0':
        this.friendlyText = 'Cosmetic';
        break;
      default:
        this.friendlyText = this.text;
        break;
    }
  }
}
