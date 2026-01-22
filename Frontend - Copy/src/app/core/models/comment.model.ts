export interface Comment {
  id: string;
  content: string;
  createdBy: string;
  createdAt: string;
  parentCommentId?: string | null;
  replies: Comment[];
}
