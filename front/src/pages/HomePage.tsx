import {Button} from "@/components/ui/button.tsx";
import {useAppDispatch} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";

const HomePage = () => {
    const dispatch = useAppDispatch();
    return(
        <>
            <h1>HomePage</h1>
            <Button onClick={() => dispatch(openModal())}>gfgafgf</Button>

        </>
    )
}

export default HomePage