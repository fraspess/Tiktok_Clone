import type {VideoDto} from "@/types/Video.ts";


// hardcode videos to test front
export const MOCK_VIDEOS: VideoDto[] = [
    {
        id: "mock-1",
        videoUrl: "https://developer.mozilla.org/shared-assets/videos/flower.mp4",
        description: "Тестове відео №1 для перевірки UI #mock #test",
        hashTags: ["mock", "test"],
        thumbnailUrl: "https://storage.googleapis.com/gtv-videos-bucket/sample/images/BigBuckBunny.jpg",
        likeCount: 1240,
        commentsCount: 84,
        favoriteCount: 312,
        viewCount: 15600,
        isFavorited: false,
        isLiked: false,
        author: {id: "mock-author-1", username: "big_buck_bunny", avatar: null},
        createdAt: new Date().toISOString(),
    },
    {
        id: "mock-2",
        videoUrl: "https://developer.mozilla.org/shared-assets/videos/friday.mp4",
        description: "Тестове відео №2 для перевірки UI #mock",
        hashTags: ["mock"],
        thumbnailUrl: "https://storage.googleapis.com/gtv-videos-bucket/sample/images/ElephantsDream.jpg",
        likeCount: 890,
        commentsCount: 41,
        favoriteCount: 120,
        viewCount: 9800,
        isFavorited: true,
        isLiked: true,
        author: {id: "mock-author-2", username: "elephants_dream", avatar: null},
        createdAt: new Date().toISOString(),
    }
];
