export interface Analysis {
  summary: string;
  sentiment: 'positive' | 'neutral' | 'negative';
  priority: string;
  nextAction: string;
}
