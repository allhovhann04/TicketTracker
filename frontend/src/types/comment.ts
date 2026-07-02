export interface Comment {
  id: string
  ticketId: string
  authorUserId: string
  body: string
  createdAt: string
}

export interface CreateCommentRequest {
  body: string
}
