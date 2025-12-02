import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiServiceService } from '../../services/api-service.service';
import { LoaderComponent } from '../../components/loader/loader.component';
import { TagComponent } from '../../components/tag/tag.component';

@Component({
  selector: 'app-feedback-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent, TagComponent],
  templateUrl: './feedback-detail.component.html',
  styleUrls: ['./feedback-detail.component.scss'],
})
export class FeedbackDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private api = inject(ApiServiceService);

  loading = false;
  error: string | null = null;
  feedback: any = null;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Invalid feedback ID.';
      return;
    }

    this.fetchDetails(id);
  }

  fetchDetails(id: string) {
    this.loading = true;

    this.api.getFeedback(id).subscribe({
      next: (res) => {
        this.feedback = res.data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load feedback details.';
        this.loading = false;
      },
    });
  }

  goBack() {
    history.back();
  }
}
