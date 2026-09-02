import {useTranslation} from "react-i18next";
import {useInfiniteFypFollowing} from "@/hooks/useInfiniteFypFollowing.ts";
import VideoFeedList from "@/components/feed/VideoFeedList.tsx";

const FollowingVideoFeed = () => {
    const {t} = useTranslation();
    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteFypFollowing(5);

    return (
        <VideoFeedList
            videos={videos}
            loadMore={loadMore}
            hasNext={hasNext}
            isFetching={isFetching}
            error={error}
            emptyMessage={t("feed.emptyFollowing")}
            loadingMessage={t("feed.loading")}
        />
    );
};

export default FollowingVideoFeed;