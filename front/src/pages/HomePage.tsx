import VideoFeed from "@/components/feed/VideoFeed.tsx";
import mascot from "@/assets/mascot-full.png";

const HomePage = () => {
    return (
        <div className="relative h-full w-full overflow-hidden bg-black">
            <VideoFeed/>

            <img
                src={mascot}
                alt=""
                className="pointer-events-none absolute -bottom-16 -right-2 z-20 w-[520px] max-w-[60vw] select-none opacity-90"
            />
        </div>
    )
}

export default HomePage