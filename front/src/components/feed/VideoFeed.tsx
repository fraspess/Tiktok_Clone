import {useTranslation} from "react-i18next";
import {useInfiniteFyp} from "@/hooks/useInfiniteFyp.ts";
import VideoFeedList from "@/components/feed/VideoFeedList.tsx";

const VideoFeed = () => {
    const {t} = useTranslation();
    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteFyp(5);

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

export default VideoFeed;