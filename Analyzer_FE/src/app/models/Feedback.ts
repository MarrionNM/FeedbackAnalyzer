export interface Feedback {
  id: string;
  text: string;
  email?: string | null;
  createdAt: string;

  analysis?: {
    summary: string;
    sentiment: string;
    priority: string;
    nextAction: string;
  };

  Tags: string[];
}
