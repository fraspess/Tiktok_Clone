import type {ReactNode} from "react";

interface PageTransitionProps {
    children: ReactNode;
}

const PageTransition = ({children}: PageTransitionProps) => {
    return (
        <> {children} </>
        // <motion.div
        //     variants={pageVariants}
        //     initial="initial"
        //     animate="animate"
        //     exit="exit"
        //     transition={{duration: 0.22, ease: [0.4, 0, 0.2, 1]}}
        //     className="h-full w-full"
        // >
        //     {children}
        // </motion.div>
    );
};

export default PageTransition;