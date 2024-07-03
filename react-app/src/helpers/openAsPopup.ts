interface Options {
  name?: string
  width?: number
  height?: number
  focus?: boolean
}

/**
 * Open url as popup
 *
 * @param {string} url - url
 * @param {Options}  options - options
 * @return {Window | null} popup - window
 */
function openAsPopup(url: string, options?: Options): Window | null {
  const defaults: Options = {
    name: '_blank',
    width: 500,
    height: 700,
    focus: true,
  }

  options = options ? { ...defaults, ...options } : defaults

  const left = screen.width / 2 - (options.width || 0) / 2
  const top = screen.height / 2 - (options.height || 0) / 2
  const popup = window.open(
    url,
    options.name,
    `width=${options.width},scrollbars=1,resizable=yes,height=${options.height},top=${top},left=${left}`,
  )

  if (options.focus && popup && popup.focus) {
    popup.focus()
  }

  return popup
}

export default openAsPopup
