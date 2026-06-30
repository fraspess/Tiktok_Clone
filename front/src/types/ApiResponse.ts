export interface ApiResponse<T> {
    data: T,
    message: string | null,
    errors: string | null,
    code: string | null,
    fieldErrors: Record<string, string[]> | null,
    isSuccess: boolean,
}