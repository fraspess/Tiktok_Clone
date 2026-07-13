export interface PaginationMetadata {
    currentPage: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
}

export interface PagedResult<T> {
    items: T[];
    metadata: PaginationMetadata;
}
