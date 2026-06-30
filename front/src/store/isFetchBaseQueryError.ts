import type {FetchBaseQueryError} from "@reduxjs/toolkit/query";

function isFetchBaseQueryError(error: unknown): error is FetchBaseQueryError {
    return typeof error === "object" && error != null && "status" in error;
}

export default isFetchBaseQueryError;