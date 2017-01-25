! function (e, n, t) {
    function o(e, n) {
        return typeof e === n
    }

    function r() {
        var e, n, t, r, s, i, a;
        for (var l in m)
            if (m.hasOwnProperty(l)) {
                if (e = [], n = m[l], n.name && (e.push(n.name.toLowerCase()), n.options && n.options.aliases && n.options.aliases.length))
                    for (t = 0; t < n.options.aliases.length; t++) e.push(n.options.aliases[t].toLowerCase());
                for (r = o(n.fn, "function") ? n.fn() : n.fn, s = 0; s < e.length; s++) i = e[s], a = i.split("."), 1 === a.length ? y[a[0]] = r : (!y[a[0]] || y[a[0]] instanceof Boolean || (y[a[0]] = new Boolean(y[a[0]])), y[a[0]][a[1]] = r), v.push((r ? "" : "no-") + a.join("-"))
            }
    }

    function s(e) {
        var n = g.className,
            t = y._config.classPrefix || "";
        if (w && (n = n.baseVal), y._config.enableJSClass) {
            var o = new RegExp("(^|\\s)" + t + "no-js(\\s|$)");
            n = n.replace(o, "$1" + t + "js$2")
        }
        y._config.enableClasses && (n += " " + t + e.join(" " + t), w ? g.className.baseVal = n : g.className = n)
    }

    function i(e, n) {
        return !!~("" + e).indexOf(n)
    }

    function a() {
        return "function" != typeof n.createElement ? n.createElement(arguments[0]) : w ? n.createElementNS.call(n, "http://www.w3.org/2000/svg", arguments[0]) : n.createElement.apply(n, arguments)
    }

    function l() {
        var e = n.body;
        return e || (e = a(w ? "svg" : "body"), e.fake = !0), e
    }

    function f(e, t, o, r) {
        var s, i, f, u, d = "modernizr",
            c = a("div"),
            p = l();
        if (parseInt(o, 10))
            for (; o--;) f = a("div"), f.id = r ? r[o] : d + (o + 1), c.appendChild(f);
        return s = a("style"), s.type = "text/css", s.id = "s" + d, (p.fake ? p : c).appendChild(s), p.appendChild(c), s.styleSheet ? s.styleSheet.cssText = e : s.appendChild(n.createTextNode(e)), c.id = d, p.fake && (p.style.background = "", p.style.overflow = "hidden", u = g.style.overflow, g.style.overflow = "hidden", g.appendChild(p)), i = t(c, e), p.fake ? (p.parentNode.removeChild(p), g.style.overflow = u, g.offsetHeight) : c.parentNode.removeChild(c), !!i
    }

    function u(e) {
        return e.replace(/([A-Z])/g, function (e, n) {
            return "-" + n.toLowerCase()
        }).replace(/^ms-/, "-ms-")
    }

    function d(n, o) {
        var r = n.length;
        if ("CSS" in e && "supports" in e.CSS) {
            for (; r--;)
                if (e.CSS.supports(u(n[r]), o)) return !0;
            return !1
        }
        if ("CSSSupportsRule" in e) {
            for (var s = []; r--;) s.push("(" + u(n[r]) + ":" + o + ")");
            return s = s.join(" or "), f("@supports (" + s + ") { #modernizr { position: absolute; } }", function (e) {
                return "absolute" == getComputedStyle(e, null).position
            })
        }
        return t
    }

    function c(e) {
        return e.replace(/([a-z])-([a-z])/g, function (e, n, t) {
            return n + t.toUpperCase()
        }).replace(/^-/, "")
    }

    function p(e, n, r, s) {
        function l() {
            u && (delete S.style, delete S.modElem)
        }
        if (s = !o(s, "undefined") && s, !o(r, "undefined")) {
            var f = d(e, r);
            if (!o(f, "undefined")) return f
        }
        for (var u, p, m, h, y, v = ["modernizr", "tspan"]; !S.style;) u = !0, S.modElem = a(v.shift()), S.style = S.modElem.style;
        for (m = e.length, p = 0; p < m; p++)
            if (h = e[p], y = S.style[h], i(h, "-") && (h = c(h)), S.style[h] !== t) {
                if (s || o(r, "undefined")) return l(), "pfx" != n || h;
                try {
                    S.style[h] = r
                } catch (e) { }
                if (S.style[h] != y) return l(), "pfx" != n || h
            }
        return l(), !1
    }
    var m = [],
        h = {
            _version: "3.3.1",
            _config: {
                classPrefix: "",
                enableClasses: !0,
                enableJSClass: !0,
                usePrefixes: !0
            },
            _q: [],
            on: function (e, n) {
                var t = this;
                setTimeout(function () {
                    n(t[e])
                }, 0)
            },
            addTest: function (e, n, t) {
                m.push({
                    name: e,
                    fn: n,
                    options: t
                })
            },
            addAsyncTest: function (e) {
                m.push({
                    name: null,
                    fn: e
                })
            }
        },
        y = function () { };
    y.prototype = h, y = new y;
    var v = [],
        g = n.documentElement,
        w = "svg" === g.nodeName.toLowerCase(),
        C = {
            elem: a("modernizr")
        };
    y._q.push(function () {
        delete C.elem
    });
    var S = {
        style: C.elem.style
    };
    y._q.unshift(function () {
        delete S.style
    });
    h.testProp = function (e, n, o) {
        return p([e], t, n, o)
    };
    y.addTest("svg", !!n.createElementNS && !!n.createElementNS("http://www.w3.org/2000/svg", "svg").createSVGRect), r(), s(v), delete h.addTest, delete h.addAsyncTest;
    for (var b = 0; b < y._q.length; b++) y._q[b]();
    e.Modernizr = y
}(window, document);