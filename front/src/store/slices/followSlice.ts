import {createSlice, type PayloadAction} from "@reduxjs/toolkit";

interface FollowState {
    overrides: Record<string, boolean>;
}

const initialState: FollowState = {overrides: {}};

const followSlice = createSlice({
    name: "follow",
    initialState,
    reducers: {
        setFollowStatus: (state, action: PayloadAction<{ userId: string; isFollowing: boolean }>) => {
            state.overrides[action.payload.userId] = action.payload.isFollowing;
        },
    },
});

export const {setFollowStatus} = followSlice.actions;
export default followSlice.reducer;