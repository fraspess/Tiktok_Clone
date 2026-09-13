import {motion} from "framer-motion";
import type {ReactNode} from "react";

interface PageTransitionProps {
    children: ReactNode;
}

const pageVariants = {
    initial: {opacity: 0, y: 8},
    animate: {opacity: 1, y: 0},
    exit: {opacity: 0, y: -8},
};

const PageTransition = ({children}: PageTransitionProps) => {
    return (
        <motion.div
            variants={pageVariants}
            initial="initial"
            animate="animate"
            exit="exit"
            transition={{duration: 0.22, ease: [0.4, 0, 0.2, 1]}}
            className="h-full w-full"
        >
            {children}
        </motion.div>
    );
};

export default PageTransition;