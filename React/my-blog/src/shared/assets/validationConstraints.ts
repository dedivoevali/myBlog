export const UsernameValidationConstraints = {
    Regexp: /^(?=.{3,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$/,
    MaxLength: 20,
    MinLength: 3
}

export const PasswordValidationConstraints = {
    MinLength: 6,
    MaxLength: 20
}

export const FirstnameLastnameConstraints = {
    Regexp: /^([A-Z][a-z]*$)/,
    MinLength: 2,
    MaxLength: 20
}

export const PostValidationConstraints = {
    TitleMaxLength: 30,
    ContentMaxLength: 10000,
    TopicMaxLength: 30
}

export const CommentValidationConstraints = {
    ContentMaxLength: 1000
}

export const commonValidationConstraints = {
    isAlphaNumeric: /^[a-zA-Z0-9_]+$/
}