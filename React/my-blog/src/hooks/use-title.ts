import { useEffect } from "react";
import { PAGE_TITLE_PREFIX } from "../shared/config";

export function useTitle(title: string, deps: React.DependencyList = []) {
    useEffect(() => {
        const previousTitle = document.title;
        document.title = PAGE_TITLE_PREFIX + ' | ' + title;
        return () => {
            document.title = previousTitle;
        }
    }, [...deps])
}