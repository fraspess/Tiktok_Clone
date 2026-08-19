export interface InitUploadData {
    url: string;
    uploadToken: string;
}

export interface InitUploadRequest {
    contentType: string;
}

export interface CompleteUploadData {
    token: string;
    description: string | null;
}