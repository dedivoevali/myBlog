import { Theme, createTheme } from "@mui/material";

export const applicationTheme: Partial<Theme> = createTheme({
    typography: {
        fontFamily: "Onest, sans-serif"
    },
    cssVariables: true
})