export interface CommentModel {
    id: number,
    registrationDate: string,
    postId: number,
    postTitle: string,
    authorId: number,
    authorUsername: string,
    authorInitials: string,
    content: string
}