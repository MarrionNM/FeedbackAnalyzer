import { Routes } from '@angular/router';
import { FeedbackListComponent } from './pages/feedback-list/feedback-list.component';
import { SubmitFeedbackComponent } from './pages/submit-feedback/submit-feedback.component';
import { FeedbackDetailComponent } from './pages/feedback-detail/feedback-detail.component';
import { NotFoundComponent } from './components/not-found/not-found.component';

export const routes: Routes = [
  { path: '', redirectTo: 'feedback', pathMatch: 'full' },

  { path: 'feedback', component: FeedbackListComponent },
  { path: 'feedback/:id', component: FeedbackDetailComponent },

  { path: 'create', component: SubmitFeedbackComponent },

  {
    path: '**',
    component: NotFoundComponent,
  },
];
