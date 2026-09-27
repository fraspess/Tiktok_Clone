import {useParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {useSharedVideoFeed} from "@/hooks/useSharedVideoFeed.ts";
import VideoFeedList from "@/components/feed/VideoFeedList.tsx";

const SharedVideoPage = () => {
    const {t} = useTranslation();
    const {videoId} = useParams<{ videoId: string }>();
    const {videos, loadMore, hasNext, isFetching, error} = useSharedVideoFeed(videoId, 5);

    return (
        <VideoFeedList
            videos={videos}
            loadMore={loadMore}
            hasNext={hasNext}
            isFetching={isFetching}
            error={error}
            emptyMessage={t("feed.empty")}
            loadingMessage={t("feed.loading")}
        />
    );
};

export default SharedVideoPage;