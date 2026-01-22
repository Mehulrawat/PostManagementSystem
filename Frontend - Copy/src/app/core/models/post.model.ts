export enum PostCategory {
  Issue = 0,
  Complaint = 1,
  LostAndFound = 2,
  HelpRequest = 3,
  Announcement = 4,
  Other = 5
}

export enum PostStatus {
  Draft = 0,
  PendingApproval = 1,
  Approved = 2,
  Resolved = 3,
  Rejected = 4,
  Closed = 5
}

export interface Post {
  id: string;
  title: string;
  content: string;
  category: PostCategory;
  status: PostStatus;
  createdBy: string;
  assignedTo?: string | null;
  createdAt: string;
}
