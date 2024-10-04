const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

function isEmail(input: string): boolean {
  if (input.trim() === '') {
    return false
  }

  return emailRegex.test(input)
}

export default isEmail
