import {useEffect, useState, type RefObject} from "react";

interface UseIntersectionObserverOptions {
    root?: RefObject<Element | null>;
    rootMargin?: string;
    threshold?: number | number[];
}

// root?.current читається всередині ефекту (а не при рендері), тому актуальний
// DOM-вузол контейнера буде використаний, навіть якщо він з'явився вже після монтування.
export function useIntersectionObserver(
    targetRef: RefObject<Element | null>,
    {root, rootMargin, threshold}: UseIntersectionObserverOptions = {}
): boolean {
    const [isIntersecting, setIsIntersecting] = useState(false);

    useEffect(() => {
        const node = targetRef.current;
        if (!node) {
            return;
        }

        const observer = new IntersectionObserver(
            ([entry]) => setIsIntersecting(entry.isIntersecting),
            {root: root?.current ?? null, rootMargin, threshold}
        );

        observer.observe(node);
        return () => observer.disconnect();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [targetRef.current, root?.current, rootMargin, JSON.stringify(threshold)]);

    return isIntersecting;
}
