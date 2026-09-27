import VideoFeed from "@/components/feed/VideoFeed.tsx";

const HomePage = () => {
    return (
        <div className="relative h-full w-full overflow-hidden bg-black">
            <VideoFeed/>

            {/*<img*/}
            {/*    src={mascot}*/}
            {/*    alt=""*/}
            {/*    loading="lazy"*/}
            {/*    className="pointer-events-none absolute -bottom-16 -right-2 z-20 hidden w-[520px] max-w-[60vw] select-none opacity-90 md:block"*/}
            {/*/>*/}
        </div>
    )
}

export default HomePage