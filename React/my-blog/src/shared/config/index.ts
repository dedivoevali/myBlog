export const API_URL = process.env.REACT_APP_API_URL;

export const PAGE_TITLE_PREFIX = process.env.REACT_APP_SITE_TITLE_PREFIX;

export const JwtTokenKeyName = "token";

export const UserIdTokenKeyName = "userId"

export const DefaultPageSize = 5

export const DefaultAvatarGroupMaxLength = 5

export const MaxAvatarSizeBytes = 100 * 1024; // 100KB synchronized with backend formal restrictions

export * from "./theme"