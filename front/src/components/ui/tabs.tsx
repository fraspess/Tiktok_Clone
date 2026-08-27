import {createContext, useContext, useState} from "react";
import type {ReactNode} from "react";
import {cn} from "@/lib/utils.ts";

interface TabsContextValue {
    value: string;
    setValue: (value: string) => void;
}

const TabsContext = createContext<TabsContextValue | null>(null);

function useTabsContext() {
    const ctx = useContext(TabsContext);
    if (!ctx) {
        throw new Error("Tabs.* повинні бути всередині <Tabs>");
    }
    return ctx;
}

interface TabsProps {
    defaultValue?: string;
    value?: string;
    onValueChange?: (value: string) => void;
    className?: string;
    children: ReactNode;
}

export function Tabs({defaultValue, value, onValueChange, className, children}: TabsProps) {
    const [internalValue, setInternalValue] = useState(defaultValue ?? "");
    const currentValue = value ?? internalValue;

    const setValue = (next: string) => {
        if (value === undefined) {
            setInternalValue(next);
        }
        onValueChange?.(next);
    };

    return (
        <TabsContext.Provider value={{value: currentValue, setValue}}>
            <div className={className}>{children}</div>
        </TabsContext.Provider>
    );
}

export function TabsList({className, children}: { className?: string; children: ReactNode }) {
    return (
        <div role="tablist" className={cn("inline-flex items-center gap-1 border-b border-neutral-800", className)}>
            {children}
        </div>
    );
}

export function TabsTrigger({value, className, children}: { value: string; className?: string; children: ReactNode }) {
    const {value: activeValue, setValue} = useTabsContext();
    const isActive = activeValue === value;

    return (
        <button
            type="button"
            role="tab"
            aria-selected={isActive}
            onClick={() => setValue(value)}
            className={cn(
                "border-b-2 px-4 py-2.5 text-sm font-medium transition-colors",
                isActive ? "border-white text-white" : "border-transparent text-muted-foreground hover:text-foreground",
                className
            )}
        >
            {children}
        </button>
    );
}

export function TabsContent({value, className, children}: { value: string; className?: string; children: ReactNode }) {
    const {value: activeValue} = useTabsContext();
    if (activeValue !== value) return null;
    return <div className={cn("h-full w-full", className)}>{children}</div>;
}